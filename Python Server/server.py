import socket
import google.generativeai as genai 
import threading
genai.configure(api_key="AIzaSyBg2hw222D0cgT0PaOfEcvQBpJTz2k0nMw")


model = genai.GenerativeModel("gemini-1.5-flash")



HOST = '127.0.0.1'  # Indirizzo locale
PORT = 65432        # Porta

def handle_client(conn, addr):
    print(f"Connesso da {addr}")
    while True:
        try:
            data = conn.recv(1024)
            if not data:
                break
            question = data.decode()
            print(f"Ricevuta domanda: {question}")

            #Utilizza Gemini per generare la risposta
            response = model.generate_content(question)
            string = response.text
            conn.sendall(string.encode())

        except ConnectionResetError:
            print(f"Connessione con {addr} terminata")
            break

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.bind((HOST, PORT))
    s.listen()
    print('Server in ascolto su', HOST, PORT)

    while True:
        conn, addr = s.accept()
        thread = threading.Thread(target=handle_client, args=(conn, addr))
        thread.start()