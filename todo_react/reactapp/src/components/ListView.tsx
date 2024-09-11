import { useGetTodoItems } from "../api/todoQueries"
import AddListItem from "./AddListItem";
import ListItem from "./ListItem";

interface Props {
    selectedList: string
}

function ListView(props: Props) {
    const { selectedList } = props
    const {data: todoItems} = useGetTodoItems(selectedList);

    return (
        <div className="w-auto">
            <div className="text-center">
                <h3 className="font-dosis text-4xl">Viewing {selectedList}</h3>
            </div>
            <div className="flex flex-col justify-items-start p-2 space-y-0.5">
            {todoItems?.map((item) => (
                <ListItem todo={item} key={item.id}/>
            ))}
            <AddListItem/>
            </div>
        </div>
    )
}

export default ListView
