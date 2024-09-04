import React, { useState } from "react";
import { Typography, Row, Col, message } from "antd";
import SeoRankingForm from "../components/SeoRankingForm";
import SeoRankingResults from "../components/SeoRankingResults";
import { getSeoRanking } from "../apis/seoRankingApi";
import { ISeoRankingSearchEngineResponse } from "../apis/dtos/ISeoRankingSearchEngineResponse";

const { Title } = Typography;

function SeoRanking() {
  const [loading, setLoading] = useState(false);
  const [results, setResults] = useState<ISeoRankingSearchEngineResponse[]>([]);

  const handleSubmit = async (values: {
    targetUrl: string;
    keyword: string;
    searchEngines: number[];
  }) => {
    setLoading(true);
    setResults([]);
    try {
      const result = await getSeoRanking(values);
      setResults(result);
      message.success("SEO ranking fetched successfully");
    } catch (error) {
      message.error("Failed to fetch SEO ranking. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Row justify="center" style={{ paddingTop: "40px" }}>
      <Col xs={20} sm={20} md={16} lg={12} xl={12}>
        <div className="SeoRanking">
          <Title
            level={1}
            style={{ textAlign: "center", marginBottom: "24px" }}
          >
            SEO Ranking
          </Title>
          <SeoRankingForm onSubmit={handleSubmit} loading={loading} />
        </div>
        {results.length > 0 && <SeoRankingResults results={results} />}
      </Col>
    </Row>
  );
}

export default SeoRanking;
