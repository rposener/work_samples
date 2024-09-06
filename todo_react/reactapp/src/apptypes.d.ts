type CreateTodoItem = {
    todoList: string;
    description: string;
    dueDate: string;
    reminderDays: number;
    isComplete: boolean;
}

type CreateTodoResponse = {
    id: string;
}