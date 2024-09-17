import { useState } from "react";
import { useAddTodo } from "../api/todoQueries";

interface AddListProps {
    setState: React.Dispatch<React.SetStateAction<string>>
}
function AddList({ setState }: AddListProps) {
    const [todoList, setTodoList] = useState("");
    const [description, setDescription] = useState("");
    const addTodoMutation = useAddTodo();

    const addTodoList = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        addTodoMutation.mutate({
            todoList,
            description
        }, {
            onSuccess: () => {
                setState(todoList);
                setDescription("");
            }
        })
    }

    return (
        <>
            <div className="text-center mb-3">
                <h3 className="font-dosis text-4xl">Add a New List</h3>
            </div>
            <form className="flex flex-col mx-5" onSubmit={addTodoList}>
                <div className="flex text-nowrap flex-row mb-3">
                    <label htmlFor="list" className="me-2">Todo List</label>
                    <input type="text" name="list" autoComplete="false" aria-autocomplete="none" value={todoList} onChange={e => setTodoList(e.target.value)} className="w-full mr-2" placeholder="type a new list name..." />
                </div>
                <div className="flex text-nowrap flex-row mb-3">
                    <label htmlFor="description" className="me-2">First item in the list</label>
                    <input type="text" name="description" autoComplete="false" aria-autocomplete="none" value={description} onChange={e => setDescription(e.target.value)} className="w-full mr-2" placeholder="type a new item..." />
                </div>
                <div className="ms-auto">
                    <button className="border-blue-900 border px-2 bg-blue-400 from-neutral-50 font-bold" type="submit">Save</button>
                </div>
            </form>
        </>
    )
}
export default AddList