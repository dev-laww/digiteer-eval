import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'
import { Button } from '../ui/button'
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '../ui/form'
import { useRegister } from '../../hooks/useAuthApi'

const schema = z.object({
  email: z.string().email('Valid email required'),
  password: z.string().min(6, 'At least 6 characters')
})

export default function RegisterForm({ onSuccess }) {
  const form = useForm({
    resolver: zodResolver(schema),
    defaultValues: { email: '', password: '' }
  })
  const registerMutation = useRegister()

  const onSubmit = (values) => {
    registerMutation.mutate(values, {
      onSuccess: () => {
        onSuccess?.()
      }
    })
  }

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="grid gap-4">
        <FormField
          control={form.control}
          name="email"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Email</FormLabel>
              <FormControl>
                <input type="email" placeholder="you@example.com" className="w-full px-3 py-2 border rounded-md" {...field} />
              </FormControl>
              <FormDescription>We will create your account using this email.</FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="password"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Password</FormLabel>
              <FormControl>
                <input type="password" className="w-full px-3 py-2 border rounded-md" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <Button type="submit" disabled={registerMutation.isPending}>
          {registerMutation.isPending ? 'Creating account…' : 'Create account'}
        </Button>
        {registerMutation.isError && (
          <p className="text-sm text-destructive">{registerMutation.error?.response?.data?.message || 'Registration failed'}</p>
        )}
      </form>
    </Form>
  )
}


