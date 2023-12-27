const getUserLoggedInMixin = {
    data() {
        return {
            userLoggedIn: null
        };
    },
    methods: {
        async getUserLoggedIn() {
            axios.get('/GetUserLoggedIn')
                .then(response => {
                    this.userLoggedIn = response.data.sessionUserLoggedIn;
                })
                .catch(error => {
                    console.error('Error fetching userLoggedIn:', error);
                });
        }
    },
    created() {
    }
};

export default getUserLoggedInMixin;