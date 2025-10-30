import LoginForm from '../components/auth/LoginForm.jsx'
import { useAuthStore } from '../store/auth.js'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import { useEffect } from 'react'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../components/ui/card.jsx'

export default function Login() {
    const isAuthenticated = useAuthStore((s) => s.isAuthenticated || !!s.token)
    const navigate = useNavigate()
    useEffect(() => {
        if (isAuthenticated) navigate('/')
    }, [ isAuthenticated, navigate ])

    if (isAuthenticated) return <Navigate to='/' replace />

    return (
        <div className='grid place-items-center px-4'>
            <Card className='w-full max-w-md'>
                <CardHeader>
                    <CardTitle className='text-2xl'>Welcome back</CardTitle>
                    <CardDescription>Sign in to continue</CardDescription>
                </CardHeader>
                <CardContent>
                    <div className='grid gap-4'>
                        <LoginForm onSuccess={ () => navigate('/') } />
                        <p className='text-sm text-center'>
                            No account? <Link className='text-primary underline' to='/register'>Create one</Link>
                        </p>
                    </div>
                </CardContent>
            </Card>
        </div>
    )
}


