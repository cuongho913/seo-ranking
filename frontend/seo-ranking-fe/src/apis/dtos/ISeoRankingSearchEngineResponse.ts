export interface ISeoRankingSearchEngineResponse {
  searchEngine: string;
  result: ISeoRankingResult[];
}

export interface ISeoRankingResult {
  url: string;
  index: number;
}
