import { create } from 'zustand'

const TOKEN_STORAGE_KEY = 'auth_token'

export const useAuthStore = create((set, get) => ({
  token: typeof localStorage !== 'undefined' ? localStorage.getItem(TOKEN_STORAGE_KEY) : null,
  isAuthenticated: false,
  setToken: (token) => {
    if (token) {
      localStorage.setItem(TOKEN_STORAGE_KEY, token)
    } else {
      localStorage.removeItem(TOKEN_STORAGE_KEY)
    }
    set({ token, isAuthenticated: !!token })
  },
  clear: () => {
    localStorage.removeItem(TOKEN_STORAGE_KEY)
    set({ token: null, isAuthenticated: false })
  }
}))


