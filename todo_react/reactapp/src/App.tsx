import { useState } from 'react'
import Sidebar from './components/Sidebar';
import ListView from './components/ListView';
import AddList from "./components/AddList";

function App() {
  const [selectedList, selectList] = useState("");

  return (
    <>
      <div className='h-lvh flex flex-row content-stretch'>
        <div className="basis-1/4 p-2 border-solid bg-gray-200 h-100">
          <Sidebar selectList={selectList} selectedList={selectedList}/>
        </div>
        <div className="basis-3/4">
          {(selectedList !== "") && (<ListView selectedList={selectedList}/>)}
                  {(selectedList === "") && (<AddList selectList={selectList} />)}
        </div>
      </div>
    </>
  )
}

export default App
