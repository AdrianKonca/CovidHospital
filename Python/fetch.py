import pandas as pd
import requests
import os.path

DATA_URL = "https://covid.ourworldindata.org/data/owid-covid-data.csv"
RAW_FILE_PATH = "data/raw_file.csv"

def fetch_raw_data_if_missing():
    if os.path.isfile(RAW_FILE_PATH):
        return
    file_request = requests.get(DATA_URL, allow_redirects=True)
    with open(RAW_FILE_PATH, 'wb') as raw_file: 
        raw_file.write(file_request.content)

fetch_raw_data_if_missing()