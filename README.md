# deepl-translator-cli

A small command-line tool that translates text using the [DeepL API](https://www.deepl.com/translator), written in C# (.NET 8).

## Features

- Translate text straight from the command line, or interactively if no text is given.
- Ships as a single self-contained binary — no .NET runtime required on the target machine.
- Clear error messages for common API issues (missing/invalid key, rate limits, plan limits, server errors).

## Requirements

- A [DeepL API key](https://www.deepl.com/pro-api) (works with the free-tier endpoint out of the box).
- To build from source: [.NET 8 SDK](https://dotnet.microsoft.com/download).

## Installation

### Option 1: Build with the provided script (Linux + Windows, from WSL)

```bash
git clone https://github.com/Lunevolab/deepl-translator-cli.git
cd deepl-translator-cli
./build-all.sh
```

This publishes self-contained single-file binaries for both platforms and installs them:
- Linux binary → `/usr/local/bin/translate`
- Windows binary → `/mnt/c/Tools/translate.exe`

### Option 2: Build manually for your platform

```bash
dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishTrimmed=true /p:PublishSingleFile=true
# or: -r win-x64
```

The binary will be in `bin/Release/net8.0/<runtime>/publish/`.

## Setup

Set your DeepL API key as an environment variable:

**Linux / macOS**
```bash
export DEEPL_API_KEY="your-api-key-here"
```
Add this line to your `~/.bashrc` or `~/.zshrc` to make it permanent.

**Windows (PowerShell)**
```powershell
setx DEEPL_API_KEY "your-api-key-here"
```

## Usage

```bash
# Translate text passed as arguments
translate How do you do?

# No arguments — prompts for input interactively
translate
Enter text to translate: How do you do?
```

> **Note:** the target language is currently fixed to English (`EN`) in code. Support for choosing a target language via a flag is on the roadmap.

## Error handling

The tool surfaces clear messages for the most common DeepL API responses, including missing/invalid API key, rate limiting, forbidden/not-found errors, and plan translation limits reached.

## License

_Not yet specified._

## Author

[Lunevolab](https://github.com/Lunevolab)
