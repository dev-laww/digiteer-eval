import './App.css'
import Tasks from "./Tasks"
import { BrowserRouter, Link, Navigate, Route, Routes } from 'react-router-dom'
import Login from './pages/Login.jsx'
import Register from './pages/Register.jsx'
import { useAuthStore } from './store/auth.js'
import ProtectedRoute from './components/ProtectedRoute'
import { Button } from './components'

function App() {
    const token = useAuthStore((s) => s.token)
    const isAuthed = !!token

    return (
        <BrowserRouter>
            <div className='app container min-h-screen mx-auto p-4 flex flex-col'>
                <header className='flex items-center justify-between'>
                    <Link to='/' className='text-3xl font-semibold'>📝 React Task Evaluator</Link>
                    <nav className='flex items-center gap-4 text-sm'>
                        { !isAuthed ? (
                            <>
                                <Link className='underline' to='/login'>Login</Link>
                                <Link className='underline' to='/register'>Register</Link>
                            </>
                        ) : (
                            <>
                                <Link className='underline' to='/'>Tasks</Link>
                                <Button
                                    variant='outline'
                                    onClick={ () => {
                                        useAuthStore.getState().clear()
                                    } }
                                >
                                    Logout
                                </Button>
                            </>
                        ) }
                    </nav>
                </header>
                <main className='grow'>
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
