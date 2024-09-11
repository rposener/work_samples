interface ListItemParams {
  todo: TodoItem
}

function ListItem({todo}: ListItemParams) {
  return (
    <div className="border border-black rounded-sm">{todo.description}</div>
  )
}
export default ListItem