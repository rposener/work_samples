import axios from "axios";

export const client = axios.create({
    baseURL: 'https://localhost:7092/',
    headers: {
        'Content-Type': 'application/json'
    }
})