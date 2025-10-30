import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { Toaster } from './ui/sonner';

const queryClient = new QueryClient({
    defaultOptions: {
        queries: {
            staleTime: 5 * 1000,
            refetchOnWindowFocus: false,
            retry: 1
        }
    }
})

export function Providers({ children }) {
    return (
        <QueryClientProvider client={ queryClient }>
            { children }
            <Toaster />
        </QueryClientProvider>
    )
}

// eslint-disable-next-line react-refresh/only-export-components
export { queryClient }


