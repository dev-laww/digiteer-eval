const THEME_STORAGE_KEY = 'theme-preference'

export function getSystemTheme() {
    if (typeof window === 'undefined') return 'light'
    return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'
}

export function getStoredTheme() {
    if (typeof window === 'undefined') return null
    try {
        return localStorage.getItem(THEME_STORAGE_KEY)
    } catch {
        return null
    }
}

export function applyTheme(theme) {
    if (typeof document === 'undefined') return
    document.documentElement.dataset.theme = theme
}

export function setTheme(theme) {
    applyTheme(theme)
    try {
        localStorage.setItem(THEME_STORAGE_KEY, theme)
    } catch {
        // empty
    }
}

export function initTheme() {
    const stored = getStoredTheme()
    const initial = stored || getSystemTheme()
    applyTheme(initial)
    return initial
}

export function toggleTheme() {
    const current = typeof document !== 'undefined' && document.documentElement.dataset.theme
    const next = current === 'dark' ? 'light' : 'dark'
    setTheme(next)
    return next
}


