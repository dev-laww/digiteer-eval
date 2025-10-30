import { useMutation } from '@tanstack/react-query'
import api from '../api/axios'
import { useAuthStore } from '../store/auth'

export function useLogin() {
    const setToken = useAuthStore((s) => s.setToken)
    return useMutation({
        mutationKey: [ 'auth', 'login' ],
        mutationFn: async ({ email, password }) => {
            const res = await api.post('/auth/login', { email, password })
            return res.data
        },
        onSuccess: (data) => {
            const token = data?.data?.token || data?.data?.Token || data?.token
            if (token) setToken(token)
        }
    })
}

export function useRegister() {
    const setToken = useAuthStore((s) => s.setToken)
    return useMutation({
        mutationKey: [ 'auth', 'register' ],
        mutationFn: async ({ email, password }) => {
            const res = await api.post('/auth/register', { email, password })
            return res.data
        },
        onSuccess: (data) => {
            const token = data?.data?.token || data?.data?.Token || data?.token
            if (token) setToken(token)
        }
    })
}


