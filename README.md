# PhonePad Challenge

An implementation of the classic "Old Phone Pad" text decoding kata: given a sequence of key presses from an old T9-style phone keypad, decode the resulting text.

The solution is a .NET class library with unit tests, exposed through a minimal ASP.NET Core Web API and consumed by an interactive React frontend that simulates the physical keypad.

## How it works

Each numeric key cycles through a set of letters as it's pressed repeatedly, following the classic phone keypad layout:

| Key | Letters |
| --- | ------- |
| 0   | (space) |
| 1   | *(none)* |
| 2   | A B C |
| 3   | D E F |
| 4   | G H I |
| 5   | J K L |
| 6   | M N O |
| 7   | P Q R S |
| 8   | T U V |
| 9   | W X Y Z |

Special characters in the input sequence:

- `#` — Send: commits the current key sequence and ends input processing.
- `*` — Backspace: cancels the character currently being cycled, or deletes the last committed character.
- ` ` (space) — Pause: commits the current key sequence and resets, allowing the next key press to start a new character (used to type consecutive letters on the same key, e.g. `"222"` → `"C"`, `"22 2"` → `"BA"`).

### Examples

| Input | Output |
| ----- | ------ |
| `"33#"` | `"E"` |
| `"227*#"` | `"B"` |
| `"4433555 555666#"` | `"HELLO"` |
| `"8 88777444666*664#"` | `"TURING"` |

## Project structure

```
PhonePadChallenge/
├── src/
│   ├── PhonePad/            # Core decoding logic (Keypad.OldPhonePad)
│   ├── PhonePad.Api/        # Minimal ASP.NET Core Web API wrapping the decoder
│   └── phonepad-ui/         # React + Vite frontend that simulates a phone keypad
└── tests/
    └── PhonePad.Tests/      # xUnit tests for the decoding logic
```

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (for the frontend)

## Getting started

### Run the tests

```bash
dotnet test
```

### Run the API

```bash
dotnet run --project src/PhonePad.Api
```

The API starts at `http://localhost:5150` and exposes:

```
POST /api/decode
Content-Type: application/json

{ "input": "4433555 555666#" }
```

Response:

```json
{ "output": "HELLO" }
```

### Run the frontend

```bash
cd src/phonepad-ui
npm install
npm run dev
```

Open the URL Vite prints (typically `http://localhost:5173`) and use the on-screen keypad. Key presses are batched and sent to the API when you press `#`.

> The frontend currently points at `http://127.0.0.1:5150` for the API — make sure the API is running first.

## Core logic

The decoding algorithm lives in [`src/PhonePad/Keypad.cs`](src/PhonePad/Keypad.cs) as a single static method:

```csharp
string Keypad.OldPhonePad(string input)
```

It has no external dependencies, so it can be reused independently of the API or UI.

## AI usage

This project was built with AI assistance. The prompt/chat log is available here: [Gemini share link](https://share.gemini.google/KK0uvreQlYhR).
