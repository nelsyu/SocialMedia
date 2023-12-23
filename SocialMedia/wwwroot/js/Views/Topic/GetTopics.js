const appGetTopics = Vue.createApp({
    data() {
        return {
            topics: [],
            selectedTopicId: ''
        };
    },
    methods: {
        async getTopics() {
            const response = await fetch('/GetTopics');
            const data = await response.json();

            this.topics = data.topicsVM;
        }
    },
    created() {
    }
});