async function checkLogin() {
    await $.ajax({
        url: '/User/IsLoggedIn',
        success: function (data) {
            if (!data) {
                alert("Please log in to your account first.");
            }
        },
        error: function (error) {
            console.error('Error checking login status:', error);
        }
    });
}