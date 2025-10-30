import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import api from '../api/axios'

const tasksKey = [ 'tasks' ]

export function useTasks() {
    return useQuery({
        queryKey: tasksKey,
        queryFn: async () => {
            const res = await api.get('/tasks')
            return res.data
        }
    })
}

export function useCreateTask() {
    const qc = useQueryClient()
    return useMutation({
        mutationKey: [ ...tasksKey, 'create' ],
        mutationFn: async ({ title }) => {
            const res = await api.post('/tasks', { title })
            return res.data
        },
        onSuccess: async () => {
            await qc.invalidateQueries({ queryKey: tasksKey })
        }
    })
}

export function useUpdateTask() {
    const qc = useQueryClient()
    return useMutation({
        mutationKey: [ ...tasksKey, 'update' ],
        mutationFn: async ({ id, ...body }) => {
            const res = await api.put(`/tasks/${ id }`, body)
            return res.data
        },
        onSuccess: async () => {
            await qc.invalidateQueries({ queryKey: tasksKey })
        }
    })
}

export function useDeleteTask() {
    const qc = useQueryClient()
    return useMutation({
        mutationKey: [ ...tasksKey, 'delete' ],
        mutationFn: async ({ id }) => {
            const res = await api.delete(`/tasks/${ id }`)
            return res.data
        },
        onSuccess: async () => {
            await qc.invalidateQueries({ queryKey: tasksKey })
        }
    })
}


