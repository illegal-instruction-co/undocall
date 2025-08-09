# undocall / twinapi-core_bi-change-appstate

Small reverse of an undocumented path in **twinapi-core.dll** that ends up talking to the **BiSrv** RPC service.

---

## tl;dr
Traced `twinapi-core.dll!BiChangeApplicationStateForPackageNameForUser` to an internal RPC interface (IID below).  
Built a C# client and confirmed the call path. It enforces caller identity correctly; cross-user attempts hit token checks and fail.  
Fun trip, no CVE.

---

## what this is

- **Client-side stub** in `twinapi-core.dll`:
  ```c
  __int64 __fastcall BiChangeApplicationStateForPackageNameForUser(
      void* Sid,            // a1: pointer to SID
      __int64 PackageArg,   // a2: points to a wide string (package full name), validated
      int State,            // a3: enum in practice, sent as int
      _BYTE* OutFlag        // a4: optional 1-byte flag out
  )
  ```
- Builds an **ncalrpc** binding, sets security (packet privacy), binds to an internal RPC interface, and issues an `NdrClientCall3` with **opnum = 1**.
- On `RPC_S_NO_ENDPOINT_FOUND (1753)`, calls `BipWaitForService()` then retries bind.

---

## key reverse notes

- Uses `RpcBindingCreateW` with:
  - `Template.ProtocolSequence = 3` (local RPC / ALPC)
  - `Security.AuthnLevel = 6` (packet privacy)
  - `Security.AuthnSvc   = 20`
- Binds to an interface descriptor at `unk_1801EB0D0` → resolves to:
  ```
  8bfc3be1-6def-4e2d-af74-7c47cd0ade4a
  ```
- Parameters marshalled:
  - SID + length
  - PackageFullName (wide string)
  - State (int)
  - OutFlag (byte out)
- Return: **NTSTATUS** as int.

---

## server side

- Implemented in **BiSrv** (`bisrv.dll`), same IID.
- Endpoints from endpoint mapper:
  - Dynamic LRPC names (`LRPC-...`)
  - Named ones like `umpo`, `actkernel`
- Service impersonates caller and validates token/session.

---

## reproduction (C# client)

- Generated a C# proxy from the interface, called **opnum 1** (`PsmBiExtNotifyAppState`).
- **Current user SID** → call succeeds, state changes.
- **Different user SID** → rejected with:
  ```
  0xC0000068 (STATUS_INVALID_LOGON_SESSION)
  ```
- RPC runtime uses the current thread/process token for auth.  
  To change identity, impersonate before binding.

---

## status / security impact

- **By design** — identity checks block cross-user access.
- No CVE.

---

## how I got here

1. Found `BiChangeApplicationStateForPackageNameForUser` in **twinapi-core.dll**.
2. Followed binding → `RpcBindingCreateW` → `RpcBindingBind` → extracted IID.
3. IID: `8bfc3be1-6def-4e2d-af74-7c47cd0ade4a`
4. Built managed client, discovered endpoints, confirmed param contract.

---

## environment / tools

- Win10/11 retail
- IDA, ProcMon, ETW traces
- Managed RPC stubs via **NtCoreLib**

---

## notes & dead ends

- Cross-user → blocked by token checks.
- Retry logic on 1753 → standard service-wait behavior.
- Auth settings match expected secure defaults.

---

## ideas for future digs

- Enumerate other opnums (`PsmBiExt*`).
- Map enum semantics for State.
- Compare endpoint names across builds.

---

## disclaimers

Research only.  
If you find something I missed, prove me wrong — I’ll update.

---

## sample code

See `src/`:
- `Cl.cs`: generated RPC stubs
- `Program.cs`: harness to resolve endpoints and call opnum 1

Requires running as the target user or legitimate impersonation.
