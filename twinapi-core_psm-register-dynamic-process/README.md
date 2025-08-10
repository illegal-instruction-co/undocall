# undocall / twinapi-core_psm-register-dynamic-process

Small reverse of an undocumented RPC call path in the **umpo** endpoint, exercised with automated state fuzzing against multiple open UWP and desktop processes.

---

## tl;dr

Traced an internal RPC opnum to the **umpo** endpoint.  
Built a C# client to invoke the call with varying `State` values against all running processes that expose the endpoint.  
Observed consistent input validation; no cross-user bypass or privilege escalation found.

---

## what this is

- **RPC client harness** that:
  1. Queries the endpoint mapper for `umpo`.
  2. Enumerates running processes with an open connection to that endpoint.
  3. Calls the target opnum for each process using multiple `State` values.

- **State fuzz set** used:
  ```
  0, 1, 2, -1, 32767, -32768, 65535
  ```
  These were chosen to cover normal, boundary, and out-of-range values.

---

## key reverse notes

- RPC binding:
  - **ProtocolSequence**: `ncalrpc` (ALPC transport)
  - **AuthnLevel**: `packet privacy` (6)
  - **AuthnSvc**: `20` (NTLM/SSPI)
- The call marshals:
  - Process identity (from PID)
  - Integer `State` value

- All cross-user attempts are subject to server-side impersonation checks.

---

## test results

Example run:
```
[*] Endpoint: ncalrpc:[umpo]

=== Notepad (PID 25060) ===
[+] RPC returned NTSTATUS=0xC0000034

=== Microsoft.Msn.News (PID 36292) ===
[+] RPC returned NTSTATUS=0x124

...
```

- Most responses were `STATUS_OBJECT_NAME_NOT_FOUND (0xC0000034)` or `STATUS_INVALID_PARAMETER`.
- One UWP app returned `0x124` (Win32-style error), worth noting but not exploitable in this context.
- No security attribute or capability changes were observed between calls.

---

## server-side behavior

- Implements strict token/session validation.
- Rejects mismatched user tokens before processing `State`.
- No difference in behavior across fuzzed values, other than expected parameter rejection.

---

## status / security impact

- **By design** — server validates caller identity and rejects unauthorized state changes.
- No indication of privilege escalation, sandbox escape, or cross-user state manipulation.
- No CVE.

---

## how I got here

1. Identified opnum usage within the `umpo` RPC service.
2. Generated a managed RPC client with **NtCoreLib**.
3. Added automated enumeration of all connected processes exposing the endpoint.
4. Fuzzed `State` parameter across the process list.
5. Compared security attributes and capabilities between calls.

---

## environment / tools

- Windows 10/11 retail builds
- IDA Pro for static analysis
- ProcMon for runtime observation
- Managed RPC stubs via **NtCoreLib**
- Custom C# harness for enumeration + fuzzing

---

## notes & dead ends

- No bypass of server-side access control discovered.
- Non-NTSTATUS return (`0x124`) on one UWP process is anomalous but non-impactful.
- No hidden side effects detected on target processes.

---

## ideas for future digs

- Extend fuzzing to other parameters or structures used by the target opnum.
- Attempt impersonation or token swapping prior to RPC bind.
- Cross-reference opnum behavior across OS versions.

---

## disclaimers

Research only.  
This was done on fully patched retail builds.  
If future OS versions alter the parameter parsing, re-test for impact.
