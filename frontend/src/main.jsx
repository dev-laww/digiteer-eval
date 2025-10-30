import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import { initTheme } from './lib/theme.js'
import App from './App.jsx'
import { Providers } from './components/'

// Initialize theme before rendering to avoid FOUC
initTheme()

createRoot(document.getElementById('root')).render(
    <StrictMode>
        <Providers>
            <App />
        </Providers>
    </StrictMode>
)
