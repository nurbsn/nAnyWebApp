# Skrypt do generowania produkcyjnej paczki Android App Bundle (.aab) dla Google Play Console
param(
    [string]$KeystoreFile = "nanywebapp-upload.keystore",
    [string]$KeyAlias = "nanywebapp",
    [string]$Password = ""
)

$ErrorActionPreference = "Stop"

$scriptDir = if (-not [string]::IsNullOrWhiteSpace($PSScriptRoot)) { $PSScriptRoot } else { Split-Path -Parent $MyInvocation.MyCommand.Path }
if ([string]::IsNullOrWhiteSpace($scriptDir)) { $scriptDir = $PWD.Path }

$keystoreFullPath = if ([System.IO.Path]::IsPathRooted($KeystoreFile)) { $KeystoreFile } else { Join-Path $scriptDir $KeystoreFile }
$projectPath = Join-Path $scriptDir "nAnyWebApp\nAnyWebApp.csproj"

Write-Host "=== nAnyWebApp - Budowanie paczki Google Play (AAB) ===" -ForegroundColor Cyan

# Lokalizacja keytool (kompatybilna z roznymi wersjami JDK / PowerShell 5.1 i 7+)
$keytoolCandidates = @(
    "C:\Program Files\Android\openjdk\jdk-21.0.8\bin\keytool.exe",
    "C:\Program Files (x86)\Android\openjdk\jdk-21.0.8\bin\keytool.exe"
)

$keytoolPath = ""
foreach ($candidate in $keytoolCandidates) {
    if (Test-Path $candidate) {
        $keytoolPath = $candidate
        break
    }
}

if ([string]::IsNullOrWhiteSpace($keytoolPath)) {
    $cmd = Get-Command "keytool.exe" -ErrorAction SilentlyContinue
    if ($cmd) {
        $keytoolPath = $cmd.Source
    }
}

if (-not (Test-Path $keystoreFullPath)) {
    Write-Host "`nNie znaleziono pliku klucza: $keystoreFullPath" -ForegroundColor Yellow
    Write-Host "Generowanie nowego klucza podpisu (Upload Keystore)..." -ForegroundColor Green
    
    if ([string]::IsNullOrWhiteSpace($Password)) {
        $secPwd = Read-Host -Prompt "Podaj bezpieczne haslo dla nowego klucza podpisu" -AsSecureString
        $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($secPwd)
        $Password = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
    }

    if ([string]::IsNullOrWhiteSpace($keytoolPath) -or (-not (Test-Path $keytoolPath))) {
        Write-Error "Nie znaleziono narzedzia keytool.exe. Upewnij sie, ze zainstalowano pakiet OpenJDK."
        exit 1
    }

    & $keytoolPath -genkeypair -v -keystore $keystoreFullPath -alias $KeyAlias -keyalg RSA -keysize 2048 -validity 10000 `
        -storepass $Password -keypass $Password -dname "CN=nAnyWebApp, O=gfmm, C=EU"
        
    Write-Host "Utworzono klucz: $keystoreFullPath (Alias: $KeyAlias)" -ForegroundColor Green
    Write-Host "UWAGA: Zachowaj ten plik i haslo w bezpiecznym miejscu! Bedzie potrzebny do kazdej kolejnej aktualizacji w Google Play." -ForegroundColor Red
}

if ([string]::IsNullOrWhiteSpace($Password)) {
    $secPwd = Read-Host -Prompt "Podaj haslo do magazynu kluczy $keystoreFullPath" -AsSecureString
    $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($secPwd)
    $Password = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
}

# Weryfikacja hasła do magazynu kluczy przed rozpoczęciem kompilacji
if (-not [string]::IsNullOrWhiteSpace($keytoolPath) -and (Test-Path $keytoolPath)) {
    Write-Host "`nWeryfikacja hasla do magazynu kluczy..." -ForegroundColor Cyan
    $verifyOutput = & $keytoolPath -list -keystore $keystoreFullPath -storepass $Password 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "BLAD: Podane haslo do magazynu kluczy jest niepoprawne!" -ForegroundColor Red
        Write-Host "Upewnij sie, ze wpisujesz dokladnie to samo haslo, ktore zostalo uzyte przy tworzeniu pliku." -ForegroundColor Yellow
        exit 1
    }
    Write-Host "Haslo prawidlowe!" -ForegroundColor Green
}

# Czyszczenie zablokowanych lub przestarzałych plików obj/Release i AAB
Write-Host "`nCzyszczenie tymczasowych plikow kompilacji..." -ForegroundColor Gray
$objRelease = Join-Path $scriptDir "nAnyWebApp\obj\Release"
if (Test-Path $objRelease) {
    Remove-Item -Path $objRelease -Recurse -Force -ErrorAction SilentlyContinue
}

$binAndroidRelease = Join-Path $scriptDir "nAnyWebApp\bin\Release\net9.0-android"
if (Test-Path $binAndroidRelease) {
    Get-ChildItem -Path $binAndroidRelease -Filter "*.aab" -Recurse -ErrorAction SilentlyContinue | Remove-Item -Force
}

Write-Host "`nKompilacja i publikacja do Android App Bundle (AAB)..." -ForegroundColor Cyan
Write-Host "Klucz: $keystoreFullPath" -ForegroundColor Gray
Write-Host "Alias: $KeyAlias" -ForegroundColor Gray

dotnet publish "$projectPath" -f net9.0-android -c Release `
    -p:AndroidPackageFormat=aab `
    -p:AndroidKeyStore=true `
    -p:AndroidSigningKeyStore="$keystoreFullPath" `
    -p:AndroidSigningKeyAlias=$KeyAlias `
    -p:AndroidSigningKeyPass=$Password `
    -p:AndroidSigningStorePass=$Password

$publishDir = Join-Path $scriptDir "nAnyWebApp\bin\Release\net9.0-android\publish"
$outputAab = Get-ChildItem -Path $publishDir -Filter "*Signed.aab" -ErrorAction SilentlyContinue | Select-Object -First 1

if ($outputAab) {
    Write-Host "`n========================================================" -ForegroundColor Green
    Write-Host "SUKCES! Gotowa, produkcyjnie podpisana paczka AAB:" -ForegroundColor Green
    Write-Host $outputAab.FullName -ForegroundColor Yellow
    Write-Host "========================================================`n" -ForegroundColor Green
    Write-Host "Mozesz teraz wgrac ten plik bezposrednio do Google Play Console." -ForegroundColor Cyan
} else {
    $fallbackAab = Get-ChildItem -Path $binAndroidRelease -Filter "*.aab" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($fallbackAab) {
        Write-Host "`nPaczka wygenerowana w:" -ForegroundColor Green
        Write-Host $fallbackAab.FullName -ForegroundColor Yellow
    }
}
