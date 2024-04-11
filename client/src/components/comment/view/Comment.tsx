import React, { useEffect, useState } from "react";
import { MDXEditor } from "@mdxeditor/editor";
import "@mdxeditor/editor/style.css";
import IComment from "../../../interfaces/comment/IComment";
import PostHeading from "../../post/heading/PostHeading";
import useAuth from "../../../contexts/use_auth/UseAuth";
import GetProfilePictureByEmailService from "../../../services/users/profile/GetProfilePictureService";

const Comment: React.FC<{ comment: IComment }> = ({
  comment: { Author, Content },
}) => {
  const { email } = useAuth();
  const [imageOfCommentAuthor, setImageOfCommentAuthor] = useState<string>("");
  const [isDeleteCommentAvailable, setIsDeleteCommentAvailable] =
    useState<boolean>(false);

  useEffect(() => {
    if (Author === email) {
      setIsDeleteCommentAvailable(true);
    }

    const fetch = async () => {
      // fetch profile picture
      const picture: string = await GetProfilePictureByEmailService(Author);
      setImageOfCommentAuthor(picture);
    };

    fetch();
  }, [Author, email]);

  const handleDeleteComment = () => {
    // Add your logic here to delete the comment
    console.log("Delete comment logic goes here");
  };

  return (
    <div className="relative bg-white rounded-lg ml-4">
      <form className="space-y-6 p-4 border border-gray-200 rounded-xl mb-2">
        {/* Delete button */}
        {isDeleteCommentAvailable && (
          <button
            onClick={handleDeleteComment}
            className="absolute top-0 right-0 p-2 mr-2 h-10 w-10 pt-4 text-red-600 font-semibold hover:text-red-500"
          >
            {/* SVG Trash Icon */}
            <svg
              viewBox="0 0 24 24"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M4 7H20"
                stroke="currentColor"
                strokeWidth={2}
                strokeLinecap="round"
                strokeLinejoin="round"
              />
              <path
                d="M6 10L7.70141 19.3578C7.87432 20.3088 8.70258 21 9.66915 21H14.3308C15.2974 21 16.1257 20.3087 16.2986 19.3578L18 10"
                stroke="currentColor"
                strokeWidth={2}
                strokeLinecap="round"
                strokeLinejoin="round"
              />
              <path
                d="M9 5C9 3.89543 9.89543 3 11 3H13C14.1046 3 15 3.89543 15 5V7H9V5Z"
                stroke="currentColor"
                strokeWidth={2}
                strokeLinecap="round"
                strokeLinejoin="round"
              />
            </svg>
          </button>
        )}
        <div>
          <PostHeading
            imageBlobUrl={imageOfCommentAuthor}
            author={Author.split("@")[0]}
            isCommentHeading={true}
          />
          <div className="mt-1">
            <MDXEditor
              readOnly
              markdown={Content}
              placeholder="Add a comment"
              className="min-h-px w-full focus:outline-none rounded-lg focus:ring-primary-500 focus:border-primary-500"
            />
          </div>
        </div>
      </form>
    </div>
  );
};

export default Comment;
