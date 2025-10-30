import axios from 'axios';

const api = axios.create({
    baseURL: import.meta.env.VITE_API_BASE_URL
});

api.interceptors.request.use((config) => {
    try {
        const token = typeof localStorage !== 'undefined' ? localStorage.getItem('auth_token') : null
        if (token) {
            config.headers = { ...(config.headers || {}), Authorization: `Bearer ${ token }` }
        }
    } catch {
        /* empty */
    }
    return config
})

api.interceptors.response.use(
    (res) => res,
    (error) => {
        const status = error?.response?.status
        if (status === 401) {
            try {
                localStorage.removeItem('auth_token')
            } catch {
                /* empty */
            }
        }
        return Promise.reject(error)
    }
)

export default api;
