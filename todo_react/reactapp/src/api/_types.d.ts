type ApiProblem = {
    errors?: {[key:string]: string[]}
    detail?: string;
    status: number;
    title: string;
    type: string;
}

type NewTodo = {
    todoList:string,
    description: string,
    dueDate?: string,
    reminderDays?: number,
    idCompleted?: boolean
}

type NewTodoResponse = {
    id: string
}

type TodoItem = {
    id: string,
    description: string,
    dueDate?: string,
    reminderDays?: number,
    idCompleted?: boolean
}