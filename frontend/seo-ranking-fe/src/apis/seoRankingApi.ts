import axios from "axios";
import { ISeoRankingSearchEngineResponse } from "./dtos/ISeoRankingSearchEngineResponse";
import { ISeoRankingRequest } from "./dtos/ISeoRankingRequest";

const API_BASE_URL = "http://localhost:5054/api/v1";

export async function getSeoRanking(
  params: ISeoRankingRequest
): Promise<ISeoRankingSearchEngineResponse[]> {
  try {
    const response = await axios.get(`${API_BASE_URL}/SeoRanking`, {
      params,
      paramsSerializer: {
        indexes: null,
      },
    });
    return response.data;
  } catch (error) {
    console.error("Error fetching SEO ranking:", error);
    throw error;
  }
}
