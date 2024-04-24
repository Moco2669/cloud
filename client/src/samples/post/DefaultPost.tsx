import IPost from "../../interfaces/post/view/IPost";

const emptyPost: IPost = {
    Id: '',
    Author: '',
    Title: '',
    Content: '',
    HasImage: false,
    ImageBlobUrl: '',
    Comments: [],
  };
  
  export default emptyPost;
  