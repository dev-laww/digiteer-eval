import { useEffect, useMemo, useState } from 'react'
import { Button, Card, CardContent, CardHeader, CardTitle } from '../index.js'
import { useCreateTask, useDeleteTask, useTasks, useUpdateTask } from '../../hooks/useTasksApi.js'

function TasksCard() {
    const { data, isLoading, isError, refetch } = useTasks()
    const createTask = useCreateTask()
    const updateTask = useUpdateTask()
    const deleteTask = useDeleteTask()

    const tasks = useMemo(() => Array.isArray(data?.data) ? data.data : [], [data])

    const [title, setTitle] = useState('')

    useEffect(() => {
        // refetch tasks after mutations settle as a safety net
        if (createTask.isSuccess || updateTask.isSuccess || deleteTask.isSuccess) {
            // react-query invalidate runs in hooks, this ensures UI sync in edge cases
            refetch()
        }
    }, [createTask.isSuccess, updateTask.isSuccess, deleteTask.isSuccess, refetch])

    async function handleAdd(e) {
        e.preventDefault()
        const value = title.trim()
        if (!value) return
        await createTask.mutateAsync({ title: value })
        setTitle('')
    }

    function toggleDone(task) {
        updateTask.mutate({ id: task.id, isDone: !task.isDone })
    }

    function updateTitle(task, newTitle) {
        const value = newTitle.trim()
        if (value && value !== task.title) {
            updateTask.mutate({ id: task.id, title: value })
        }
    }

    function remove(id) {
        deleteTask.mutate({ id })
    }

    return (
        <Card>
            <CardHeader>
                <CardTitle>Tasks</CardTitle>
            </CardHeader>
            <CardContent>
                <form onSubmit={ handleAdd } className='flex gap-2 mb-4'>
                    <input
                        className='border rounded px-3 py-2 grow'
                        placeholder='Add a new task'
                        value={ title }
                        onChange={ (e) => setTitle(e.target.value) }
                    />
                    <Button type='submit' disabled={ createTask.isPending }>
                        { createTask.isPending ? 'Adding...' : 'Add' }
                    </Button>
                </form>

                { isLoading ? (
                    <div>Loading tasks...</div>
                ) : isError ? (
                    <div className='text-red-600'>Failed to load tasks.</div>
                ) : tasks.length === 0 ? (
                    <div className='text-slate-500'>No tasks yet. Create your first one above.</div>
                ) : (
                    <ul className='space-y-2'>
                        { tasks.map((t) => (
                            <li key={ t.id } className='flex items-center gap-3'>
                                <input
                                    type='checkbox'
                                    checked={ !!t.isDone }
                                    onChange={ () => toggleDone(t) }
                                />
                                <input
                                    className={`grow bg-transparent border-b px-1 py-1 ${ t.isDone ? 'line-through text-slate-500' : '' }`}
                                    defaultValue={ t.title }
                                    onBlur={ (e) => updateTitle(t, e.target.value) }
                                />
                                <Button
                                    variant='outline'
                                    onClick={ () => remove(t.id) }
                                    disabled={ deleteTask.isPending }
                                >
                                    Delete
                                </Button>
                            </li>
                        )) }
                    </ul>
                ) }
            </CardContent>
        </Card>
    )
}

export default TasksCard


