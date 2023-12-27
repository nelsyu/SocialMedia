const getPostsMixin = {
    data() {
        return {
            posts: [],
            currentPage: 1,
            totalPages: 1
        };
    },
    methods: {
        getPosts(postsType, id, apiPath, page) {
            axios.get(apiPath, { params: {postsType: postsType, id: id, currentPage: page } })
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
};

export default getPostsMixin;