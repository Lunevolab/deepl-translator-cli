#!/bin/bash
set -e

# Очистка старых сборок
rm -rf bin/Release/net8.0/linux-x64
rm -rf bin/Release/net8.0/win-x64
rm -rf obj/Release

echo "Старые сборки удалены."

# Публикация для Linux
echo "Сборка для Linux..."
dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishTrimmed=true /p:PublishSingleFile=true
echo "Linux сборка завершена."

# Копирование Linux бинарника
sudo cp bin/Release/net8.0/linux-x64/publish/Translator /usr/local/bin/translate
sudo chmod +x /usr/local/bin/translate
echo "Linux бинарник скопирован в /usr/local/bin/translate"

# Публикация для Windows
echo "Сборка для Windows..."
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishTrimmed=true /p:PublishSingleFile=true
echo "Windows сборка завершена."

# Копирование Windows бинарника
mkdir -p /mnt/c/Tools
cp bin/Release/net8.0/win-x64/publish/Translator.exe /mnt/c/Tools/translate.exe
echo "Windows бинарник скопирован в C:\\Tools\\translate.exe"

echo "✅ Сборка завершена для обеих платформ!"

