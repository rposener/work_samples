import { FormEventHandler, useState } from "react";
import { useAddTodo } from "../api/todoQueries"

interface AddListItemProps {
  selectedList: string
}

function AddListItem({selectedList}: AddListItemProps) {
  const addTodoMutation = useAddTodo();
  const [description, setDescription] = useState("");

  const addTodo = (e:React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    addTodoMutation.mutate({
      todoList: selectedList,
      description
    }, {
      onSuccess: () => {
        setDescription("");
      }
    })
  }

  return (
    <div className="border border-black rounded-sm">
      <form className="flex flex-row" onSubmit={addTodo}>
        <input type="text" name="description" value={description} onChange={e => setDescription(e.target.value)} className="w-full mr-2" placeholder="type a new item..." />
        <button className="border-blue-900 border px-2 bg-blue-400 from-neutral-50 font-bold" type="submit">Save</button>
        </form>
    </div>
  )
}
export default AddListItem