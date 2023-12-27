const getTopicsMixin = {
    data() {
        return {
            topics: [],
            selectedTopicId: null
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
};

export default getTopicsMixin;