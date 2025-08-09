# undocall

> *"just poking around where the docs forgot to look."*

A growing collection of reverse engineering notes and experiments on undocumented Windows APIs, RPC interfaces, and other internal mechanisms.

This repository is **not** about finding instant CVEs — it's about documenting corners of the OS that haven't been widely explored, so others don't have to start from scratch.

---

## structure

Each discovery or reversed component lives in its own folder under the project root, with:
- **Source code** (C#, C++, scripts, etc.)
- **README.md** describing the reverse process, findings, and any relevant notes
- Optional **build/run instructions** if reproduction is practical

Example:
```
twinapi-core_bi-change-appstate/
  ├─ src/
  │   ├─ cl.cs
  │   └─ program.cs
  └─ README.md
```

---

## philosophy

- **Transparency:** Share the path, not just the destination.
- **No overclaiming:** If something is *by design*, it’s labeled as such.
- **Traceability:** GUIDs, structs, and call flows are documented.
- **Modesty with a wink:** We know most of this won't break the internet — but it’s fun.

---

## disclaimers

- This repo is for **research and educational purposes only**.
- Any code here is intended to run in controlled environments.
- Do not attempt to run these tools on systems you do not own or have permission to test.

---

## future work

- Expand into more subsystems: NT internals, kernel APIs, COM/DCOM, etc.
- Build a searchable index of discovered undocumented interfaces and methods.
- Keep adding more detailed, per-target readmes.

If you find something new — PRs are welcome.
