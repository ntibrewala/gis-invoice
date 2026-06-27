import requests
import json
import urllib3
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

AUTH_URL = "https://gstsandbox.charteredinfo.com/eivital/dec/v1.04/auth"
ASP_ID = "1805053626"
PASSWORD = "Bhavya2026@"
GSTIN = "34AACCC1596Q002"
USER_NAME = "TaxProEnvPON"
EINV_PWD = "abc34*"

def get_token():
    headers = {
        "aspid": ASP_ID,
        "password": PASSWORD,
        "Gstin": GSTIN,
        "user_name": USER_NAME,
        "eInvPwd": EINV_PWD
    }
    
    response = requests.get(AUTH_URL, headers=headers, verify=False)
    print(response.text)

if __name__ == "__main__":
    get_token()
