window.initializeDialog = function () {
    const openDialogButton = document.getElementById('openDialog');
    const formDialog = document.getElementById('formDialog');
    const closeDialogButton = document.getElementById('closeDialog');

    openDialogButton.addEventListener('click', () => {
        formDialog.showModal();
    });

    closeDialogButton.addEventListener('click', () => {
        formDialog.close();
    });

    document.getElementById('userForm').addEventListener('submit', async (event) => {
        event.preventDefault();

        const formData = new FormData(event.target);
        const user = {
            firstName: formData.get('firstName'),
            lastName: formData.get('lastName'),
            email: formData.get('email')
        };

        const response = await fetch('/submitUser', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(user)
        });

        const result = await response.json();
        alert(result.message);

        if (result.success) {
            document.getElementById('formDialog').close();
        }
    });
}