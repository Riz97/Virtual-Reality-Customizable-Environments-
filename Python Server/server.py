import socket
import google.generativeai as genai 
model = genai.GenerativeModel("gemini-1.5-flash")
HOST = '127.0.0.1'  # Standard loopback interface address (localhost)
PORT = 1234        # Port to listen on (non-privileged ports are > 1023)
genai.configure(api_key="AIzaSyBg2hw222D0cgT0PaOfEcvQBpJTz2k0nMw")
print('Server in ascolto su', HOST, PORT)

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.bind((HOST,PORT))
    s.listen()
    conn, addr = s.accept()
   
    with conn:
        print('Connected by', addr)
        while True:
            data = conn.recv(1024)   
            response = model.generate_content(data.decode())
            string = response.text
            conn.sendall(string.encode())
            if not data:
                break
            

         
