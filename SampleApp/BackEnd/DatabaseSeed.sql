CREATE TABLE IF NOT EXISTS users (
    user_id INTEGER PRIMARY KEY,
    username TEXT UNIQUE NOT NULL,
    user_role TEXT NOT NULL DEFAULT 'Registered',
    password_hash TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS articles (
    article_id INTEGER PRIMARY KEY,
    title TEXT NOT NULL,
    content TEXT NOT NULL,
    author_id INTEGER NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (author_id) REFERENCES users(user_id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS comments (
    comment_id INTEGER PRIMARY KEY,
    content TEXT NOT NULL,
    article_id INTEGER NOT NULL,
    user_id INTEGER NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (article_id) REFERENCES articles(article_id) ON DELETE CASCADE,
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE
);

/* System.Data.SQLite does not like the SQLite date format */
UPDATE articles
SET created_at = strftime('%Y-%m-%d %H:%M:%S', created_at);
UPDATE comments
SET created_at = strftime('%Y-%m-%d %H:%M:%S', created_at);

/* pass: 'EditorPassword' */
INSERT INTO users (username, user_role, password_hash) VALUES ('EditorUser', 'Editor', 'BLwQ/9RYsAVAvL5LbztY+L939QN2HSJz2Ifafo50eQjRwknkRjpT+oMEGcKceoOlGRzMTQnZJcMH3XVfo6l2Zg==');
/* pass: 'JournalistPassword' */
INSERT INTO users (username, user_role, password_hash) VALUES ('JournalistUser', 'Journalist', 'nxQxcuLkJJ4cxc5nWURvD0Q/76OAYh8LWxzlyAotco+BAwGhFNXcX8OVdH5nObc+8g+hrx/IeZ/qamy3aUAPcQ==');
/* pass: 'RegisteredPassword' */
INSERT INTO users (username, user_role, password_hash) VALUES ('RegisteredUser', 'Registered', 'G+bK8hMmD1YSc/ZI5gpJ8WK/OslNUsjlc1aiEtO4QcB0LbltZ+YZAso1b2LK89TiW8fLvZduC3lm4zsgcGNh+A==');

INSERT INTO articles (title, content, author_id) VALUES ('Article 1', 'Content of article 1', 2);
INSERT INTO articles (title, content, author_id) VALUES ('Article 2', 'Content of article 2', 2);

INSERT INTO comments (content, article_id, user_id) VALUES ('Comment 1', 1, 3);
INSERT INTO comments (content, article_id, user_id) VALUES ('Comment 2', 1, 3);
