import axios, { AxiosResponse } from "axios";
import IPost from "../../../interfaces/post/view/IPost";
import { API_ENDPOINT } from "../../../App";

const GetPostByIdService = async (
    id: string | undefined,
    token: string
  ): Promise<IPost | null> => {
  
    if(!id || id === "") {
      return null;
    }

    try {
      const response: AxiosResponse = await axios.get(
        API_ENDPOINT + `post/${id}`,
        {
          headers: {
            "Authorization": `Bearer ${token}`,
          },
        }
      );
  
      if (response.status === 200 || response.status === 204) {
        return response.data;
      } else {
        return null;
      }
    } catch {
      return null;
    }
  };
  
  export default GetPostByIdService;
  