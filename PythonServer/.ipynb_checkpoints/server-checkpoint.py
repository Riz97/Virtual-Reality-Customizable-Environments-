import socket
import time
import google.generativeai as genai 
model = genai.GenerativeModel("gemini-1.0-pro")
HOST = '127.0.0.1'  # Standard loopback interface address (localhost)
PORT = 1234        # Port to listen on (non-privileged ports are > 1023)
genai.configure(api_key="AIzaSyBg2hw222D0cgT0PaOfEcvQBpJTz2k0nMw")

print("Welcome to the GEMINI Python server for this Framework")

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.bind((HOST,PORT))
    s.listen()
    conn, addr = s.accept()
    print('Server in ascolto su', HOST, PORT)
    with conn:
        print('Connected by', addr)
        while True:
            data = conn.recv(10240)
            print(data.decode())      
            if(data.decode()!= "STOP"):
                response = model.generate_content(data.decode())
                time.sleep(10)
                string = response.text
                print(string)
                conn.sendall(string.encode())
