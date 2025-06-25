import React from 'react';
import { Card, Typography, Row, Col, Button } from 'antd';
import { DownloadOutlined, BarChartOutlined } from '@ant-design/icons';

const { Title } = Typography;

const Reports: React.FC = () => {
  return (
    <div>
      <Title level={2}>Reports</Title>
      
      <Row gutter={[16, 16]}>
        <Col xs={24} sm={12} lg={8}>
          <Card>
            <Title level={4}>Employee Report</Title>
            <p>Generate comprehensive employee reports</p>
            <Button type="primary" icon={<DownloadOutlined />}>
              Generate Report
            </Button>
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={8}>
          <Card>
            <Title level={4}>Affiliation Report</Title>
            <p>View practice area affiliations</p>
            <Button type="primary" icon={<BarChartOutlined />}>
              Generate Report
            </Button>
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={8}>
          <Card>
            <Title level={4}>Analytics Dashboard</Title>
            <p>View system analytics and metrics</p>
            <Button type="primary" icon={<BarChartOutlined />}>
              View Dashboard
            </Button>
          </Card>
        </Col>
      </Row>
    </div>
  );
};

export default Reports; 