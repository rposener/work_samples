/* eslint-disable @typescript-eslint/no-explicit-any */
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { client } from './client';
import { AxiosError } from 'axios';

export const useGetTodoLists = () => {
    return useQuery<Array<string>, AxiosError<any>>({
        queryFn: async () => {
            const { data } = await client.get<Array<string>>('/todos');
            return data;
        },
        queryKey: ['todo','lists'],
        staleTime: 5 * 60 * 1000     // Check User Every 5 minutes
    });
};

export const useGetTodoItems = (list:string) => {
    return useQuery<Array<TodoItem>, AxiosError<any>>({
        queryFn: async () => {
            const { data } = await client.get<Array<TodoItem>>(`/todos/${list}`);
            return data;
        },
        enabled: !!list,
        queryKey: ['todo',list],
        staleTime: 5 * 60 * 1000     // Check User Every 5 minutes
    });
};

export const useAddTodo = () => {
    const queryClient = useQueryClient();
    return useMutation<NewTodoResponse, AxiosError<ApiProblem>, NewTodo>({
        mutationFn: async (newTodo) => {
            const post = await client.post<NewTodoResponse>(`/todos`, newTodo);
            return post.data;
        },
        onSettled: async () => {
            await queryClient.invalidateQueries({ queryKey: ["todo"] });
        }
    });
}

export const useUpdateTodo = () => {
    const queryClient = useQueryClient();
    return useMutation<TodoItem, AxiosError<ApiProblem>, TodoItemWithList>({
        mutationFn: async (todo) => {
            const post = await client.post<TodoItem>(`/todos/${todo.todoList}`, todo);
            return post.data;
        },
        onSettled: async () => {
            await queryClient.invalidateQueries({ queryKey: ["todo"] });
        }
    });
}