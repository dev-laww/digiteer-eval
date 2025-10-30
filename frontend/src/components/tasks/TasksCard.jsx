import { useMemo, useState } from 'react'
import { Button, Card, CardContent, CardHeader, CardTitle } from '../index.js'
import { useCreateTask, useDeleteTask, useTasks, useUpdateTask } from '../../hooks/useTasksApi.js'
import { Loader2, Plus, Trash2 } from 'lucide-react'

function TasksCard() {
    const { data, isLoading, isError } = useTasks()
    const createTask = useCreateTask()
    const updateTask = useUpdateTask()
    const deleteTask = useDeleteTask()

    const tasks = useMemo(() => Array.isArray(data?.data) ? data.data : [], [ data ])

    const [ title, setTitle ] = useState('')

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

    const pending = createTask.isPending || updateTask.isPending || deleteTask.isPending

    return (
        <Card>
            <CardHeader>
                <div className='flex items-center justify-between'>
                    <CardTitle className='text-xl'>Tasks</CardTitle>
                    <span className='text-sm text-muted-foreground'>{ tasks.length } total</span>
                </div>
            </CardHeader>
            <CardContent>
                <form onSubmit={ handleAdd } className='flex gap-2 mb-5'>
                    <input
                        className='h-9 grow rounded-md border border-input bg-background px-3 text-sm placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring'
                        placeholder='Add a new task'
                        value={ title }
                        onChange={ (e) => setTitle(e.target.value) }
                        aria-label='Task title'
                    />
                    <Button type='submit' size='icon' disabled={ createTask.isPending }>
                        <span className='inline-flex items-center gap-2'>
                            { createTask.isPending ? (
                                <Loader2 className='size-4 animate-spin' aria-hidden />

                            ) : (

                                <Plus className='size-4' aria-hidden />
                            ) }
                        </span>
                    </Button>
                </form>

                { isLoading ? (
                    <div className='flex items-center gap-2 text-muted-foreground'>
                        <Loader2 className='size-4 animate-spin' aria-hidden />
                        Loading tasks...
                    </div>
                ) : isError ? (
                    <div className='text-sm text-destructive'>Failed to load tasks. Try again.</div>
                ) : tasks.length === 0 ? (
                    <div className='text-sm text-muted-foreground'>
                        No tasks yet. Use the field above to add your first task.
                    </div>
                ) : (
                    <ul className='space-y-2' aria-live='polite'>
                        { tasks.map((t) => (
                            <li
                                key={ t.id }
                                className='flex items-center gap-3 rounded-md border border-border bg-card/50 p-2 px-4 transition-colors hover:bg-accent/40'
                            >
                                <input
                                    type='checkbox'
                                    checked={ !!t.isDone }
                                    onChange={ () => toggleDone(t) }
                                    aria-label={ t.isDone ? 'Mark as not done' : 'Mark as done' }
                                />
                                <input
                                    className={ `grow bg-transparent px-1 py-1 border-b border-transparent focus:border-input focus:outline-none focus:ring-0 text-sm ${ t.isDone ? 'line-through text-muted-foreground' : '' }` }
                                    defaultValue={ t.title }
                                    onBlur={ (e) => updateTitle(t, e.target.value) }
                                    aria-label='Task title'
                                />
                                <Button
                                    variant='ghost'
                                    size='icon'
                                    onClick={ () => remove(t.id) }
                                    disabled={ deleteTask.isPending }
                                    aria-label='Delete task'
                                    title='Delete'
                                >
                                    <Trash2 className='size-4' aria-hidden />
                                </Button>
                            </li>
                        )) }
                    </ul>
                ) }

                { pending && (
                    <div className='sr-only' role='status' aria-live='polite'>Updating…</div>
                ) }
            </CardContent>
        </Card>
    )
}

export default TasksCard


