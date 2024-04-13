import axios, { AxiosResponse } from "axios";
import IPost from "../../../interfaces/post/view/IPost";
import { API_ENDPOINT } from "../../../App";

const GetPostsService = async (pageNumber: number = 1, pageSize: number = 10, searchKeywords: string = ""): Promise<IPost[] | null> => {
  try {
    const response: AxiosResponse = await axios.get(API_ENDPOINT + `post/all?pageNumber=${pageNumber}&pageSize=${pageSize}&searchKeywords=${searchKeywords}`);

    if (response.status === 200 || response.status === 204) {
      return response.data;
    } else {
      return null;
    }
  } catch {
    return null;
  }
};

export default GetPostsService;
