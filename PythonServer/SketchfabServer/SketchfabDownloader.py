import requests
import os
import socket
import zipfile
import shutil

# Il tuo token di accesso Sketchfab
api_token = "vvAlrUNsMbiGG02w7So1CDE61eimIY"
HOST = '127.0.1.10'
PORT = 12321 

# Percorso base per il salvataggio dei file
base_save_path = "C:/Users/ricky/Desktop/Framework/Virtual-Reality-Customizable-Environments-/Assets/Imported/"

# Percorso della cartella dove verranno copiati i file FBX
fbx_save_path = "C:/Users/ricky/Desktop/Framework/Virtual-Reality-Customizable-Environments-/Assets/ImportedFBX/"

def get_download_link(model_uid, api_token):
    """
    Recupera il link di download del modello 3D da Sketchfab usando l'UID.
    """
    url = f"https://api.sketchfab.com/v3/models/{model_uid}/download"
    headers = {"Authorization": f"Bearer {api_token}"}

    response = requests.get(url, headers=headers)

    if response.status_code == 200:
        response_json = response.json()
        model_link = response_json.get('source', {}).get('url')
        return model_link
    else:
        print(f"Errore durante il recupero del link di download: {response.status_code}")
        return None

def create_folder_if_not_exists(path):
    """
    Crea una cartella se non esiste già.
    """
    if not os.path.exists(path):
        os.makedirs(path)

def unzip_file(zip_path, extract_to):
    """
    Estrae un file ZIP nella cartella specificata.
    """
    try:
        create_folder_if_not_exists(extract_to)

        with zipfile.ZipFile(zip_path, 'r') as zip_ref:
            zip_ref.extractall(extract_to)
    except zipfile.BadZipFile:
        print(f"Errore: il file {zip_path} non è un file ZIP valido.")
    except Exception as e:
        print(f"Errore durante l'estrazione del file ZIP: {e}")

def find_fbx_files(directory_path):
    """
    Cerca ricorsivamente i file .fbx nelle sottocartelle di una directory.
    """
    fbx_files = []
    for root, dirs, files in os.walk(directory_path):  # os.walk è ricorsivo
        for file in files:
            if file.endswith('.fbx'):
                fbx_files.append(os.path.join(root, file))  # Aggiungi il percorso completo del file .fbx
    return fbx_files

def copy_fbx_files(fbx_files, destination_directory):
    """
    Copia i file .fbx nella cartella di destinazione.
    """
    create_folder_if_not_exists(destination_directory)

    for fbx_file in fbx_files:
        destination_fbx = os.path.join(destination_directory, os.path.basename(fbx_file))  # Usa solo il nome del file per evitare conflitti

        try:
            shutil.copy(fbx_file, destination_fbx)
            print(f"File {fbx_file} copiato in {destination_directory}.")
        except Exception as e:
            print(f"Errore nel copiare {fbx_file}: {e}")

def process_nested_zips(directory_path):
    """
    Cerca ricorsivamente file .zip nelle sottocartelle, li estrae, 
    cerca file .fbx e, se non trovati, elimina la directory principale.
    """
    for root, dirs, files in os.walk(directory_path):  # Ricorsivo
        for file in files:
            if file.endswith('.zip'):
                zip_path = os.path.join(root, file)
                temp_extract_path = os.path.splitext(zip_path)[0]  # Estrai nella stessa cartella

                # Estrai il file ZIP
                unzip_file(zip_path, temp_extract_path)

                # Cerca file FBX nella directory estratta
                fbx_files = find_fbx_files(temp_extract_path)

                if fbx_files:
                    # Copia i file FBX trovati
                    copy_fbx_files(fbx_files, fbx_save_path)
                else:
                    # Nessun file FBX trovato, elimina la directory estratta
                    delete_folder_and_contents(temp_extract_path)

                # Elimina il file ZIP originale
                os.remove(zip_path)

def delete_folder_and_contents(folder_path):
    """
    Elimina la cartella e i suoi contenuti.
    """
    try:
        if os.path.exists(folder_path):
            shutil.rmtree(folder_path)
            print(f"Cartella eliminata: {folder_path}")
        else:
            print(f"Errore: La cartella {folder_path} non esiste.")
    except Exception as e:
        print(f"Errore durante l'eliminazione della cartella {folder_path}: {e}")

print("Welcome to the Sketchfab Server")

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.bind((HOST, PORT))
    s.listen()
    print('Server in ascolto su', HOST, PORT)
    conn, addr = s.accept()
    with conn:
        print('Connected by', addr)
        while True:
            data = conn.recv(102400)
            if not data:
                break
            print(data.decode())  # Uid sent by the Unity Framework Download Button
            name, uid = data.decode().split(" ", 1)

            # Assicurati che la cartella di salvataggio esista
            create_folder_if_not_exists(base_save_path)

            # Recupera il link di download per il modello
            download_url = get_download_link(uid, api_token)
            if not download_url:
                print("Download URL non trovato. Riprova.")
                continue

            zip_path = os.path.join(base_save_path, f"{name}.zip")

            # Scarica il file ZIP
            response = requests.get(download_url, stream=True)
            with open(zip_path, 'wb') as f:
                for chunk in response.iter_content(chunk_size=1024):
                    if chunk:
                        f.write(chunk)

            # Estrai il file ZIP
            extract_to = os.path.join(base_save_path, name)
            unzip_file(zip_path, extract_to)
            os.remove(zip_path)

            # Cerca e gestisci ricorsivamente file .zip e .fbx nelle sottocartelle
            process_nested_zips(extract_to)

            # Cerca file .fbx nella directory principale
            fbx_files = find_fbx_files(extract_to)
            if fbx_files:
                copy_fbx_files(fbx_files, fbx_save_path)
            else:
                # Nessun file FBX trovato, elimina la directory principale
                delete_folder_and_contents(extract_to)