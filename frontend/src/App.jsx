import './App.css'
import Tasks from './pages/Tasks'
import { BrowserRouter, Link, Navigate, Route, Routes } from 'react-router-dom'
import Login from './pages/Login'
import Register from './pages/Register'
import { useAuthStore } from './store/auth'
import ProtectedRoute from './components/ProtectedRoute'
import { Button } from './components'
import { toggleTheme } from './lib/theme.js'
import { useEffect, useState } from 'react'
import { Home, LogOut, Moon, Sun } from 'lucide-react'

function App() {
    const token = useAuthStore((s) => s.token)
    const isAuthed = !!token
    const [ theme, setTheme ] = useState('light')

    useEffect(() => {
        const current = typeof document !== 'undefined' ? document.documentElement.dataset.theme : 'light'
        setTheme(current || 'light')
    }, [])

    return (
        <BrowserRouter>
            <div className='app container min-h-screen mx-auto p-4 flex flex-col'>
                <header className='flex items-center justify-between py-4 gap-4'>
                    <Link to='/' className='flex items-center gap-2 text-2xl font-semibold'>
                        <Home className='size-6' aria-hidden />
                        <span className='hidden sm:inline'>React Task Evaluator</span>
                    </Link>
                    <nav className='flex items-center gap-3'>
                        <Button
                            variant='outline'
                            size='icon'
                            onClick={ () => setTheme(toggleTheme()) }
                            aria-label='Toggle theme'
                            title='Toggle theme'
                        >
                            { theme === 'dark' ? <Sun className='size-5' aria-hidden /> :
                                <Moon className='size-5' aria-hidden /> }
                        </Button>
                        { !isAuthed ? null : (
                            <>
                                <Button asChild variant='ghost'>
                                    <Link to='/' aria-label='Tasks' title='Tasks'>
                                        Tasks
                                    </Link>
                                </Button>
                                <Button
                                    variant='ghost'
                                    onClick={ () => {
                                        useAuthStore.getState().clear()
                                    } }
                                    aria-label='Logout'
                                    title='Logout'
                                >
                                    Logout
                                    <LogOut className='ml-2 size-4' aria-hidden />
                                </Button>
                            </>
                        ) }
                    </nav>
                </header>
                <main className='grow flex flex-col'>
                    <Routes>
                        <Route path='/login' element={ <Login /> } />
                        <Route path='/register' element={ <Register /> } />
                        <Route
                            path='/'
                            element={
                                <ProtectedRoute>
                                    <Tasks />
                                </ProtectedRoute>
                            }
                        />
                        <Route path='*' element={ <Navigate to='/' replace /> } />
                    </Routes>
                </main>
            </div>
        </BrowserRouter>
    );
}

export default App
