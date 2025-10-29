import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { urlHistoryService, UrlHistoryItem } from '../services/urlHistoryService';
import './FullHistoryPage.css';

const ITEMS_PER_PAGE = 10;

const FullHistoryPage: React.FC = () => {
    const navigate = useNavigate();
    const [history, setHistory] = useState<UrlHistoryItem[]>([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [copiedId, setCopiedId] = useState<string | null>(null);

    useEffect(() => {
        loadHistory();
    }, []);

    const loadHistory = () => {
        const items = urlHistoryService.getHistory();
        setHistory(items);
    };

    const handleCopy = async (item: UrlHistoryItem) => {
        try {
            await navigator.clipboard.writeText(item.shortUrl);
            setCopiedId(item.id);
            setTimeout(() => setCopiedId(null), 2000);
        } catch (error) {
            console.error('Failed to copy:', error);
            alert('Không thể sao chép URL');
        }
    };

    const handleDelete = (id: string) => {
        if (window.confirm('Bạn có chắc muốn xóa URL này khỏi lịch sử?')) {
            urlHistoryService.removeFromHistory(id);
            loadHistory();
        }
    };

    const handleClearAll = () => {
        if (window.confirm('Bạn có chắc muốn xóa toàn bộ lịch sử?')) {
            urlHistoryService.clearHistory();
            loadHistory();
        }
    };

    const formatDate = (dateString: string) => {
        const date = new Date(dateString);
        const now = new Date();
        const diffMs = now.getTime() - date.getTime();
        const diffMins = Math.floor(diffMs / 60000);
        const diffHours = Math.floor(diffMs / 3600000);
        const diffDays = Math.floor(diffMs / 86400000);

        if (diffMins < 1) return 'Vừa xong';
        if (diffMins < 60) return `${diffMins} phút trước`;
        if (diffHours < 24) return `${diffHours} giờ trước`;
        if (diffDays < 7) return `${diffDays} ngày trước`;

        return date.toLocaleDateString('vi-VN', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    };

    const truncateUrl = (url: string, maxLength: number = 60) => {
        if (url.length <= maxLength) return url;
        return url.substring(0, maxLength) + '...';
    };

    // Phân trang
    const totalPages = Math.ceil(history.length / ITEMS_PER_PAGE);
    const startIndex = (currentPage - 1) * ITEMS_PER_PAGE;
    const endIndex = startIndex + ITEMS_PER_PAGE;
    const currentItems = history.slice(startIndex, endIndex);

    const handlePageChange = (page: number) => {
        setCurrentPage(page);
        window.scrollTo({ top: 0, behavior: 'smooth' });
    };

    if (history.length === 0) {
        return (
            <div className="full-history-page">
                <div className="page-header">
                    <button className="back-btn" onClick={() => navigate('/')}>
                        ← Quay lại
                    </button>
                    <h1>📜 Lịch sử rút gọn URL</h1>
                </div>
                <div className="history-empty">
                    <p>📋 Chưa có lịch sử rút gọn URL</p>
                    <button className="back-to-home-btn" onClick={() => navigate('/')}>
                        🏠 Về trang chủ
                    </button>
                </div>
            </div>
        );
    }

    return (
        <div className="full-history-page">
            <div className="page-header">
                <button className="back-btn" onClick={() => navigate('/')}>
                    ← Quay lại
                </button>
                <h1>📜 Lịch sử rút gọn URL</h1>
                <button className="clear-all-btn" onClick={handleClearAll}>
                    🗑️ Xóa tất cả
                </button>
            </div>

            <div className="history-stats">
                <div className="stat-item">
                    <span className="stat-label">Tổng số URL:</span>
                    <span className="stat-value">{history.length}</span>
                </div>
                <div className="stat-item">
                    <span className="stat-label">Trang:</span>
                    <span className="stat-value">{currentPage} / {totalPages}</span>
                </div>
            </div>

            <div className="history-list-full">
                {currentItems.map((item, index) => (
                    <div key={item.id} className="history-item-full">
                        <div className="item-number">#{startIndex + index + 1}</div>
                        <div className="item-content">
                            <div className="item-urls">
                                <div className="url-row">
                                    <span className="url-label">Gốc:</span>
                                    <span className="url-text" title={item.originalUrl}>
                                        {truncateUrl(item.originalUrl)}
                                    </span>
                                </div>
                                <div className="url-row">
                                    <span className="url-label">Rút gọn:</span>
                                    <a
                                        href={item.shortUrl}
                                        target="_blank"
                                        rel="noopener noreferrer"
                                        className="url-link"
                                    >
                                        {item.shortUrl}
                                    </a>
                                </div>
                            </div>
                            <div className="item-meta">
                                <span className="item-time">{formatDate(item.createdAt)}</span>
                            </div>
                        </div>
                        <div className="item-actions">
                            <button
                                className={`copy-btn ${copiedId === item.id ? 'copied' : ''}`}
                                onClick={() => handleCopy(item)}
                            >
                                {copiedId === item.id ? '✓ Đã copy' : '📋 Sao chép'}
                            </button>
                            <button
                                className="delete-btn"
                                onClick={() => handleDelete(item.id)}
                            >
                                🗑️
                            </button>
                        </div>
                    </div>
                ))}
            </div>

            {/* Phân trang */}
            {totalPages > 1 && (
                <div className="pagination">
                    <button
                        className="page-btn"
                        onClick={() => handlePageChange(currentPage - 1)}
                        disabled={currentPage === 1}
                    >
                        ← Trước
                    </button>

                    <div className="page-numbers">
                        {Array.from({ length: totalPages }, (_, i) => i + 1).map(page => (
                            <button
                                key={page}
                                className={`page-number ${currentPage === page ? 'active' : ''}`}
                                onClick={() => handlePageChange(page)}
                            >
                                {page}
                            </button>
                        ))}
                    </div>

                    <button
                        className="page-btn"
                        onClick={() => handlePageChange(currentPage + 1)}
                        disabled={currentPage === totalPages}
                    >
                        Sau →
                    </button>
                </div>
            )}
        </div>
    );
};

export default FullHistoryPage;
