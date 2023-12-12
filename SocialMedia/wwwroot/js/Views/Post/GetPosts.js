const appGetPosts = Vue.createApp({
    data() {
        return {
            posts: [],
            currentPage: 1,
            totalPages: 1
        };
    },
    methods: {
        getPosts(postsType, userId, apiPath, page) {
            axios.get(apiPath, { params: {postsType: postsType, userId: userId, currentPage: page } })
                .then(response => {
                    this.posts = response.data.postsVM;
                    this.currentPage = response.data.currentPage;
                    this.totalPages = response.data.totalPages;
                })
                .catch(error => {
                    console.error(error);
                });
        },
        goToPostDetail(postId) {
            window.location.href = `/Post/DetailPost?postId=${postId}`;
        }
    },
    created() {
    }
});