import requests
url = "http://localhost:8082/"
data = {
    "command": "move",
    "text": "das ist ein test."
}
requests.post(url, json=data, timeout=5)