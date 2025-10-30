import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'
import { Button } from '../ui/button'
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '../ui/form'
import { useLogin } from '../../hooks/useAuthApi'
import { toast } from 'sonner';

const schema = z.object({
    email: z.email('Valid email required'),
    password: z.string().min(6, 'At least 6 characters')
})

export default function LoginForm({ onSuccess }) {
    const form = useForm({
        resolver: zodResolver(schema),
        defaultValues: { email: '', password: '' }
    })
    const login = useLogin()

    const onSubmit = (values) => {
        login.mutate(values, {
            onSuccess: () => {
                onSuccess?.()
                toast.success("Logged in successfully")
            }
        })
    }

    return (
        <Form { ...form }>
            <form onSubmit={ form.handleSubmit(onSubmit) } className='grid gap-4'>
                <FormField
                    control={ form.control }
                    name='email'
                    render={ ({ field }) => (
                        <FormItem>
                            <FormLabel>Email</FormLabel>
                            <FormControl>
                                <input type='email' placeholder='you@example.com'
                                       className='w-full px-3 py-2 border rounded-md' { ...field } />
                            </FormControl>
                            <FormDescription>Use the email you registered with.</FormDescription>
                            <FormMessage />
                        </FormItem>
                    ) }
                />
                <FormField
                    control={ form.control }
                    name='password'
                    render={ ({ field }) => (
                        <FormItem>
                            <FormLabel>Password</FormLabel>
                            <FormControl>
                                <input type='password' className='w-full px-3 py-2 border rounded-md' { ...field } />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    ) }
                />
                <Button type='submit' disabled={ login.isPending }>
                    { login.isPending ? 'Signing in…' : 'Sign in' }
                </Button>
                { login.isError && (
                    <p className='text-sm text-destructive'>{ login.error?.response?.data?.message || 'Login failed' }</p>
                ) }
            </form>
        </Form>
    )
}


