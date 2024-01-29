const getUserInfoMixin = {
    data() {
        return {
            userLoggedIn: null,
            user2: null
        };
    },
    methods: {
        async getUserLoggedIn() {
            axios.get('/GetUserInfo')
                .then(response => {
                    this.userLoggedIn = response.data.userInfo;
                })
                .catch(error => {
                    console.error('Error fetching userLoggedIn:', error);
                });
        },
        async getUserInfo(userId) {
            axios.get(`/GetUserInfo`, { params: { userId: userId } })
                .then(response => {
                    this.user2 = response.data.userInfo;
                })
                .catch(error => {
                    console.error('Error fetching userLoggedIn:', error);
                });
        }
    },
    created() {
    }
};

export default getUserInfoMixin;