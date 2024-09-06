import { useState } from 'react'

interface Props {
    selectedList: string,
    selectList: (list: string) => void;
}

function Sidebar(props: Props) {
    const { selectedList, selectList } = props
    const [todoLists, setTodoLists] = useState(["list 1", "list 2", "list 3"]);

    return (
        <nav>
            <ol role="list" className='divide-y divide-gray-400'>
                <li className="from-neutral-500 font-bold text-center">Lists Available</li>
                {todoLists.map((list) =>
                    selectedList === list ? (
                        <li key={list} className="text-white font cursor-pointer bg-gray-600 ps-2" 
                        onClick={() => selectList(list)}>{list}</li>
                    ) :
                        (
                            <li key={list} className="cursor-pointer ps-2" 
                            onClick={() => selectList(list)}>{list}</li>
                        ))}
                <li className="flex flex-col content-stretch">
                    <button type='button' 
                    onClick={() => selectList("")}
                    className='mt-3 bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 border border-blue-700 rounded'>
                        Add New List
                    </button>
                </li>
            </ol>
        </nav>
    )
}

export default Sidebar
