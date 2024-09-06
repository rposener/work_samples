interface Props {
    selectedList: string
}

function ListView(props: Props) {
    const { selectedList } = props

    return (
        <div className="w-auto">
            <div className="text-center">
                <h3 className="font-dosis text-4xl">Viewing {selectedList}</h3>
            </div>

        </div>
    )
}

export default ListView
