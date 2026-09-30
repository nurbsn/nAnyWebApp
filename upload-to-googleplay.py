import os
import sys
import glob
from google.oauth2 import service_account
from googleapiclient.discovery import build
from googleapiclient.http import MediaFileUpload

PACKAGE_NAME = "eu.gfmm.nanywebapp"
KEY_FILE = "gen-lang-client-0077146295-3ea3b88c5e9f.json"
SCOPES = ['https://www.googleapis.com/auth/androidpublisher']

def find_aab():
    # Szukamy podpisanego AAB w katalogu publish
    pattern = os.path.join("nAnyWebApp", "bin", "Release", "net9.0-android", "publish", "*Signed.aab")
    matches = glob.glob(pattern)
    if matches:
        return matches[0]
    pattern_fallback = os.path.join("nAnyWebApp", "bin", "Release", "net9.0-android", "**", "*.aab")
    matches_fallback = glob.glob(pattern_fallback, recursive=True)
    if matches_fallback:
        return matches_fallback[0]
    return None

def upload_bundle(aab_path=None, track='production', status='completed'):
    if not os.path.exists(KEY_FILE):
        print(f"BLAD: Nie znaleziono pliku klucza API: {KEY_FILE}")
        sys.exit(1)

    if not aab_path:
        aab_path = find_aab()

    if not aab_path or not os.path.exists(aab_path):
        print("BLAD: Nie znaleziono pliku AAB do wyslania!")
        print("Uruchom najpierw: .\\build-android-release.ps1, aby zbudowac paczke.")
        sys.exit(1)

    print(f"=== Automatyczny upload do Google Play Console ===")
    print(f"Plik paczki: {aab_path}")
    print(f"Rozmiar: {os.path.getsize(aab_path) / (1024*1024):.2f} MB")
    print(f"Sciezka (Track): {track}")

    try:
        credentials = service_account.Credentials.from_service_account_file(
            KEY_FILE, scopes=SCOPES
        )
        service = build('androidpublisher', 'v3', credentials=credentials)

        print("\n1. Tworzenie sesji edycji w Google Play...")
        edit = service.edits().insert(body={}, packageName=PACKAGE_NAME).execute()
        edit_id = edit['id']
        print(f"   Sesja utworzona (Edit ID: {edit_id})")

        print("\n2. Przesylanie pakietu AAB do Google Play (moze zajac kilkadziesiat sekund)...")
        media = MediaFileUpload(aab_path, mimetype='application/octet-stream', resumable=True)
        bundle_req = service.edits().bundles().upload(
            packageName=PACKAGE_NAME,
            editId=edit_id,
            media_body=media
        )
        bundle_resp = bundle_req.execute()
        version_code = bundle_resp.get('versionCode')
        sha256 = bundle_resp.get('sha256')
        print(f"   SUKCES! Pakiet przeslany! Kod wersji (VersionCode): {version_code}")
        print(f"   SHA-256: {sha256}")

        print(f"\n3. Przypisywanie wersji {version_code} do sciezki '{track}'...")
        track_body = {
            'track': track,
            'releases': [{
                'name': f"{version_code} (1.0.{version_code - 1})",
                'versionCodes': [str(version_code)],
                'status': status,
                'releaseNotes': [{
                    'language': 'pl-PL',
                    'text': 'Aktualizacja ikony programu uruchamiającego i ekranu startowego.'
                }]
            }]
        }
        service.edits().tracks().update(
            packageName=PACKAGE_NAME,
            editId=edit_id,
            track=track,
            body=track_body
        ).execute()
        print(f"   Przypisano wersje do sciezki {track} ze statusem '{status}'.")

        print("\n4. Zatwierdzanie zmian w Google Play Console (Commit)...")
        commit_resp = service.edits().commit(packageName=PACKAGE_NAME, editId=edit_id).execute()
        print("   SUKCES! Zmiany zostaly zatwierdzone w Google Play Console!")
        print("\n========================================================")
        print(f"Wersja {version_code} zostala pomyslnie wyslana i opublikowana w Google Play!")
        print("========================================================")

    except Exception as e:
        print(f"\nBLAD PODCZAS PRZESYLANIA: {e}")
        sys.exit(1)

if __name__ == '__main__':
    aab = sys.argv[1] if len(sys.argv) > 1 else None
    upload_bundle(aab)
